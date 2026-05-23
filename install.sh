#!/bin/bash

# Arrays para rastrear o status da instalação
installed_successfully=()
installation_failed=()

# Função para verificar se um comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Função para verificar se um pacote npm global está instalado
npm_global_package_exists() {
    npm list -g --depth=0 "$1" >/dev/null 2>&1
}

# Função para verificar se uma ferramenta dotnet global está instalada
dotnet_tool_exists() {
    dotnet tool list -g | grep -q "$1"
}

# --- Funções de Instalação ---

install_nvm() {
    echo "ℹ️ Verificando NVM..."
    if [ -s "$HOME/.nvm/nvm.sh" ]; then
        echo "✅ NVM já está instalado."
        export NVM_DIR="$HOME/.nvm"
        [ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
        [ -s "$NVM_DIR/bash_completion" ] && \. "$NVM_DIR/bash_completion"
        installed_successfully+=("NVM")
    else
        echo "⏳ Instalando NVM..."
        curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh | bash
        if [ $? -eq 0 ]; then
            echo "✅ NVM instalado com sucesso."
            export NVM_DIR="$HOME/.nvm"
            [ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
            [ -s "$NVM_DIR/bash_completion" ] && \. "$NVM_DIR/bash_completion"
            installed_successfully+=("NVM")
        else
            echo "❌ Erro ao instalar NVM."
            installation_failed+=("NVM")
        fi
    fi
}

install_node() {
    local version=$1
    echo "ℹ️ Verificando Node.js versão $version (via NVM)..."
    if nvm list | grep -q "v$version"; then
        echo "✅ Node.js versão $version já está instalado."
        nvm use "$version"
        installed_successfully+=("Node.js $version")
    else
        echo "⏳ Instalando Node.js versão $version via NVM..."
        nvm install "$version"
        if [ $? -eq 0 ]; then
            echo "✅ Node.js versão $version instalado com sucesso."
            nvm use "$version"
            installed_successfully+=("Node.js $version")
        else
            echo "❌ Erro ao instalar Node.js versão $version."
            installation_failed+=("Node.js $version")
        fi
    fi
}

install_angular_cli() {
    local version=$1
    local full_package_name="@angular/cli@$version"
    echo "ℹ️ Verificando Angular CLI versão $version..."
    if command_exists ng && ng version | grep -q "Angular CLI: $version"; then
        echo "✅ Angular CLI versão $version já está instalado."
        installed_successfully+=("Angular CLI $version")
    else
        echo "⏳ Instalando Angular CLI versão $version..."
        npm install -g "$full_package_name"
        if [ $? -eq 0 ]; then
            echo "✅ Angular CLI $version instalado com sucesso."
            installed_successfully+=("Angular CLI $version")
        else
            echo "❌ Erro ao instalar Angular CLI $version."
            installation_failed+=("Angular CLI $version")
        fi
    fi
}

install_dotnet_sdk() {
    local version=$1
    echo "ℹ️ Verificando .NET SDK $version..."
    if command_exists dotnet && dotnet --list-sdks | grep -q "^${version}\."; then
        echo "✅ .NET SDK $version já está instalado."
        installed_successfully+=(".NET SDK $version")
    else
        echo "⏳ Instalando .NET SDK $version..."
        if ! grep -q "dotnet/backports" /etc/apt/sources.list /etc/apt/sources.list.d/*; then
            echo "⏳ Adicionando repositório dotnet/backports..."
            gpg --keyserver keyserver.ubuntu.com --recv-keys A6A19B38D3D831EF
            gpg --export --armor A6A19B38D3D831EF | tee /usr/share/keyrings/dotnet-backports.gpg > /dev/null
            echo "deb [signed-by=/usr/share/keyrings/dotnet-backports.gpg] http://ppa.launchpad.net/dotnet/backports/ubuntu jammy main" > /etc/apt/sources.list.d/dotnet-backports.list
            apt-get update
        fi
        apt-get install -y "dotnet-sdk-${version}"
        if [ $? -eq 0 ]; then
            echo "✅ .NET SDK $version instalado com sucesso."
            installed_successfully+=(".NET SDK $version")
        else
            echo "❌ Erro ao instalar .NET SDK $version."
            installation_failed+=(".NET SDK $version")
        fi
    fi
}

install_reportgenerator() {
    local tool_name="dotnet-reportgenerator-globaltool"
    echo "ℹ️ Verificando ReportGenerator..."
    if dotnet_tool_exists "$tool_name"; then
        echo "✅ ReportGenerator já está instalado."
        installed_successfully+=("ReportGenerator")
    else
        echo "⏳ Instalando ReportGenerator..."
        dotnet tool install --global "$tool_name"
        if [ $? -eq 0 ]; then
            echo "✅ ReportGenerator instalado com sucesso."
            if [[ ":$PATH:" != *":$HOME/.dotnet/tools:"* ]]; then
                echo "🔧 Adicionando diretório de ferramentas dotnet ao PATH..."
                export PATH="$PATH:$HOME/.dotnet/tools"
                echo "📌 Para tornar esta alteração permanente, adicione 'export PATH=\"\$PATH:\$HOME/.dotnet/tools\"' ao seu ~/.bashrc ou ~/.zshrc"
            fi
            installed_successfully+=("ReportGenerator")
        else
            echo "❌ Erro ao instalar ReportGenerator."
            installation_failed+=("ReportGenerator")
        fi
    fi
}

# --- Execução ---

echo "🚀 Iniciando processo de instalação de dependências..."
apt-get update && apt-get install -y curl gnupg software-properties-common

install_nvm

if [[ " ${installed_successfully[@]} " =~ " NVM " ]]; then
    install_node "16"
    if [[ " ${installed_successfully[@]} " =~ " Node.js 16 " ]]; then
        install_angular_cli "12"
    else
        installation_failed+=("Angular CLI 12 (Node.js 16 não disponível)")
    fi
else
    installation_failed+=("Node.js 16 (NVM não disponível)")
    installation_failed+=("Angular CLI 12 (NVM não disponível)")
fi

install_dotnet_sdk "8.0"
install_dotnet_sdk "9.0"

if command_exists dotnet; then
    install_reportgenerator
else
    installation_failed+=("ReportGenerator (.NET SDK não disponível)")
fi

# --- Resumo ---

echo ""
echo "--- Resumo da Instalação ---"

if [ ${#installed_successfully[@]} -gt 0 ]; then
    echo "✅ Instalados com sucesso:"
    for item in "${installed_successfully[@]}"; do
        echo "  - $item"
    done
fi

if [ ${#installation_failed[@]} -gt 0 ]; then
    echo "❌ Falha na instalação:"
    for item in "${installation_failed[@]}"; do
        echo "  - $item"
    done
else
    echo "🎉 Todas as ferramentas foram instaladas com sucesso!"
fi

echo "--- Fim ---"
