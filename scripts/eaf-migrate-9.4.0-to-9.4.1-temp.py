#!/usr/bin/env python3
"""
Script temporário de migração EAF 9.4.0 -> 9.4.1

Atualiza projetos gerados a partir dos templates Angular e API do EAF.

Uso:
    python3 eaf-migrate-9.4.0-to-9.4.1-temp.py \
        --eaf-repo /caminho/para/EAF \
        --angular-dir /caminho/para/MyProject.UI \
        --api-dir /caminho/para/MyProject.Api

O script:
1. Gera e aplica um patch do template Angular 9.4.0 -> 9.4.1.
2. Atualiza as referências dos pacotes EAF de 9.4.0 para 9.4.1 nos .csproj do API.
3. Atualiza <Version>9.4.0</Version> para 9.4.1 no common.props do API.
4. Cria backups em <angular|api>/.eaf-migrate-backup-<timestamp>.

Recomenda-se rodar em uma branch de migração e revisar o diff antes de commitar.
"""
import argparse
import datetime
import os
import re
import shutil
import subprocess
import sys
import tempfile


def run_cmd(args, cwd=None, check=True):
    result = subprocess.run(args, cwd=cwd, capture_output=True, text=True)
    if check and result.returncode != 0:
        raise RuntimeError(f"Command failed: {' '.join(args)}\n{result.stderr}")
    return result


def backup_file(path, backup_dir):
    rel = os.path.relpath(path, os.path.dirname(backup_dir))
    backup_path = os.path.join(backup_dir, rel)
    os.makedirs(os.path.dirname(backup_path), exist_ok=True)
    shutil.copy2(path, backup_path)


def update_api_versions(api_dir, backup_dir, dry_run=False):
    changed = []
    patterns = [
        (re.compile(r'(<Version>)9\.4\.0(</Version>)'), r'\g<1>9.4.1\g<2>'),
        (re.compile(r'(Include="Eaf\.[^"]+"\s+Version=")9\.4\.0(")'), r'\g<1>9.4.1\g<2>'),
    ]

    for root, _, files in os.walk(api_dir):
        # ignore build artifacts and previous backups
        parts = os.path.normpath(root).split(os.sep)
        if any(part in parts for part in ('bin', 'obj', '.eaf-migrate-backup')):
            continue
        for name in files:
            if not (name.endswith('.csproj') or name == 'common.props'):
                continue
            file_path = os.path.join(root, name)
            with open(file_path, 'r', encoding='utf-8-sig') as f:
                content = f.read()
            new_content = content
            for pat, repl in patterns:
                new_content = pat.sub(repl, new_content)
            if new_content != content:
                if not dry_run:
                    backup_file(file_path, backup_dir)
                    with open(file_path, 'w', encoding='utf-8') as f:
                        f.write(new_content)
                changed.append(file_path)
    return changed


def generate_angular_patch(eaf_repo, patch_file, source_ref, target_ref):
    """Gera patch do template Angular source_ref -> target_ref com caminhos relativos a src/."""
    result = run_cmd(
        ['git', 'diff', f'{source_ref}..{target_ref}', '--', 'Templates/Angular/Eaf.ProjectName.UI/src'],
        cwd=eaf_repo
    )
    # Reescreve os prefixos ---/+++ de a/Templates/Angular/Eaf.ProjectName.UI/src/...
    # para a/src/... (para aplicar dentro da raiz do projeto Angular).
    transformed = re.sub(
        r'^(---|\+\+\+) ([ab])/Templates/Angular/Eaf\.ProjectName\.UI/',
        r'\1 \2/',
        result.stdout,
        flags=re.MULTILINE
    )
    with open(patch_file, 'w', encoding='utf-8') as f:
        f.write(transformed)


def list_files_in_patch(patch_file):
    files = []
    with open(patch_file, 'r', encoding='utf-8') as f:
        for line in f:
            if line.startswith('+++ '):
                # 'b/src/app/...'
                parts = line.split()
                if len(parts) >= 2 and parts[1].startswith('b/'):
                    files.append(parts[1][2:])
    return files


def apply_angular_patch(angular_dir, patch_file, backup_dir, dry_run=False):
    # Verifica se patch está disponível
    run_cmd(['which', 'patch'])

    # Backup dos arquivos que serão alterados
    if not dry_run:
        for rel in list_files_in_patch(patch_file):
            target = os.path.join(angular_dir, rel)
            if os.path.exists(target):
                backup_file(target, backup_dir)

    # Testa aplicação do patch
    dry = run_cmd(
        ['patch', '-p1', '--dry-run', '--input', patch_file],
        cwd=angular_dir,
        check=False
    )
    if dry.returncode != 0:
        print('WARNING: Angular patch cannot be applied cleanly.')
        print(dry.stdout)
        print(dry.stderr)
        return []

    if dry_run:
        print('DRY-RUN: Angular patch would apply cleanly.')
        return list_files_in_patch(patch_file)

    result = run_cmd(
        ['patch', '-p1', '--input', patch_file],
        cwd=angular_dir,
        check=False
    )
    if result.returncode != 0:
        print('ERROR: Angular patch application failed.')
        print(result.stdout)
        print(result.stderr)
        sys.exit(1)
    return list_files_in_patch(patch_file)


def main():
    parser = argparse.ArgumentParser(
        description='Migra projetos gerados do template EAF 9.4.0 para 9.4.1'
    )
    parser.add_argument('--eaf-repo', default='/home/ubuntu/repos/EAF',
                        help='Caminho do repositório EAF (com as referências source/target)')
    parser.add_argument('--angular-dir', required=False,
                        help='Caminho raiz do projeto Angular (deve conter src/)')
    parser.add_argument('--api-dir', required=False,
                        help='Caminho raiz da solução API .NET')
    parser.add_argument('--source-ref', default='9.4.0',
                        help='Referência do EAF da versão de origem (default: 9.4.0)')
    parser.add_argument('--target-ref', default='HEAD',
                        help='Referência do EAF da versão de destino (default: HEAD)')
    parser.add_argument('--dry-run', action='store_true',
                        help='Mostra o que seria alterado sem aplicar')
    args = parser.parse_args()

    if not (args.angular_dir or args.api_dir):
        parser.error('Informe pelo menos --angular-dir ou --api-dir')

    if not os.path.isdir(args.eaf_repo):
        print(f'ERROR: EAF repo not found: {args.eaf_repo}')
        sys.exit(1)

    # Verifica se as referências existem no EAF repo
    for ref in (args.source_ref, args.target_ref):
        ref_check = run_cmd(['git', 'cat-file', '-e', ref], cwd=args.eaf_repo, check=False)
        if ref_check.returncode != 0:
            print(f'ERROR: referência {ref} não encontrada no EAF repo')
            sys.exit(1)

    timestamp = datetime.datetime.now().strftime('%Y%m%d-%H%M%S')
    report = []

    if args.angular_dir:
        angular_dir = os.path.abspath(args.angular_dir)
        if not os.path.isdir(os.path.join(angular_dir, 'src')):
            print(f'ERROR: Angular dir does not contain src/: {angular_dir}')
            sys.exit(1)

        backup_dir = os.path.join(angular_dir, f'.eaf-migrate-backup-{timestamp}')
        with tempfile.NamedTemporaryFile(mode='w', suffix='.patch', delete=False) as patch_fd:
            patch_file = patch_fd.name
        try:
            generate_angular_patch(args.eaf_repo, patch_file, args.source_ref, args.target_ref)
            changed = apply_angular_patch(angular_dir, patch_file, backup_dir, args.dry_run)
            if changed:
                report.append(f'Angular: {len(changed)} arquivo(s) alterado(s) em {angular_dir}')
                report.append(f'  backup: {backup_dir}')
            else:
                report.append(f'Angular: nenhuma alteração aplicada em {angular_dir}')
        finally:
            if os.path.exists(patch_file):
                os.unlink(patch_file)

    if args.api_dir:
        api_dir = os.path.abspath(args.api_dir)
        if not os.path.isdir(api_dir):
            print(f'ERROR: API dir not found: {api_dir}')
            sys.exit(1)
        backup_dir = os.path.join(api_dir, f'.eaf-migrate-backup-{timestamp}')
        changed = update_api_versions(api_dir, backup_dir, args.dry_run)
        if changed:
            report.append(f'API: {len(changed)} arquivo(s) alterado(s) em {api_dir}')
            report.append(f'  backup: {backup_dir}')
        else:
            report.append(f'API: nenhuma referência 9.4.0 encontrada em {api_dir}')

    print('\n=== Resumo da migração ===')
    for line in report:
        print(line)
    print('\nRecomendação: revise o diff, rode dotnet build e ng build antes de commitar.')


if __name__ == '__main__':
    main()
