import { Component, Injector } from '@angular/core';
import { FeatureTreeEditModel } from '@app/admin/shared/feature-tree-edit.model';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FlatFeatureDto, NameValueDto } from '@shared/service-proxies/service-proxies';
import { ArrayToTreeConverterService } from '@shared/utils/array-to-tree-converter.service';
import { TreeDataHelperService } from '@shared/utils/tree-data-helper.service';

import { TreeNode } from 'primeng/api';

@Component({
  standalone: false,
  selector: 'feature-tree',
  templateUrl: './feature-tree.component.html',
})
export class FeatureTreeComponent extends AppComponentBase {
  _editData: FeatureTreeEditModel;

  set editData(val: FeatureTreeEditModel) {
    this._editData = val;
    this.setTreeData(val.features);
    this.setSelectedNodes(val);
  }

  treeData: any;
  selectedFeatures: TreeNode[] = [];

  constructor(
    private readonly _arrayToTreeConverterService: ArrayToTreeConverterService,
    private readonly _treeDataHelperService: TreeDataHelperService,
    injector: Injector,
  ) {
    super(injector);
  }

  setTreeData(permissions: FlatFeatureDto[]) {
    this.treeData = this._arrayToTreeConverterService.createTree(permissions, 'parentName', 'name', null, 'children', [
      {
        target: 'label',
        source: 'displayName',
      },
      {
        target: 'expandedIcon',
        value: 'fa fa-folder-open m--font-warning',
      },
      {
        target: 'collapsedIcon',
        value: 'fa fa-folder m--font-warning',
      },
      {
        target: 'expanded',
        value: true,
      },
      {
        target: 'selectable',
        value: true,
      },
    ]);
  }

  setSelectedNodes(val: FeatureTreeEditModel) {
    val.features?.forEach(feature => {
      const items = val.featureValues?.filter(f => f.name === feature.name) || [];
      if (items?.length === 1) {
        const item = items[0];
        this.setSelectedNode(item.name, item.value);
      } else {
        this.setSelectedNode(feature.name, feature.defaultValue);
      }
    });
  }

  setSelectedNode(featureName, value) {
    const node = this._treeDataHelperService.findNode(this.treeData, { data: { name: featureName } });
    if (!node) {
      return;
    }

    if (value === 'true') {
      this.addToSelection(node);
    } else if (value && value !== 'false') {
      node.value = value;
      this.addToSelection(node);
    }
  }

  private addToSelection(node) {
    if (!this.selectedFeatures.find(n => n.data.name === node.data.name)) {
      this.selectedFeatures.push(node);
    }
  }

  getGrantedFeatures(): NameValueDto[] {
    if (!this._editData.features) {
      return [];
    }

    const features: NameValueDto[] = [];

    for (const f of this._editData.features) {
      const feature = new NameValueDto();
      feature.name = f.name;
      feature.value = this.getFeatureValueByName(feature.name);

      features.push(feature);
    }

    return features;
  }

  private setDefaultValueIfNeeded(node): void {
    if (node.data.inputType.name !== 'CHECKBOX' && this.isFeatureSelected(node.data.name) && !node.value) {
      node.value = node.data.defaultValue;
    }
  }

  onDropdownChange(node) {
    this.toggleSelectionByValue(node);
  }

  private toggleSelectionByValue(node) {
    const index = this.selectedFeatures.findIndex(n => n.data.name === node.data.name);
    if (node.value) {
      if (index === -1) {
        this.selectedFeatures = [...this.selectedFeatures, node];
      }
    } else {
      if (index > -1) {
        this.selectedFeatures = this.selectedFeatures.filter(n => n.data.name !== node.data.name);
      }
    }
  }

  findFeatureByName(featureName: string): FlatFeatureDto {


    const feature = this._editData.features?.find(f => f.name === featureName);

    if (!feature) {
      eaf.log.warn('Could not find a feature by name: ' + featureName);
    }

    return feature;
  }

  findFeatureValueByName(featureName: string) {

    const feature = this.findFeatureByName(featureName);
    if (!feature) {
      return '';
    }

    const featureValue = this._editData.featureValues?.find(f => f.name === featureName);
    if (!featureValue) {
      return feature.defaultValue;
    }

    return featureValue.value;
  }

  isFeatureValueValid(featureName: string, value: string): boolean {
    const feature = this.findFeatureByName(featureName);
    if (!feature?.inputType?.validator) {
      return true;
    }

    const validator = feature.inputType.validator as any;
    if (validator.name === 'STRING') {
      return this.validateStringValue(validator, value);
    }

    if (validator.name === 'NUMERIC') {
      return this.validateNumericValue(validator, value);
    }

    return true;
  }

  private validateStringValue(validator: any, value: string): boolean {
    if (value === undefined || value === null) {
      return validator.allowNull;
    }

    if (typeof value !== 'string') {
      return false;
    }

    if (validator.minLength > 0 && value.length < validator.minLength) {
      return false;
    }

    if (validator.maxLength > 0 && value.length > validator.maxLength) {
      return false;
    }

    if (validator.regularExpression) {
      return new RegExp(validator.regularExpression).test(value);
    }

    return true;
  }

  private validateNumericValue(validator: any, value: string): boolean {
    const numValue = Number.parseInt(value);

    if (Number.isNaN(numValue)) {
      return false;
    }

    if (validator.minValue > numValue) {
      return false;
    }

    if (validator.maxValue > 0 && numValue > validator.maxValue) {
      return false;
    }

    return true;
  }

  areAllValuesValid(): boolean {
    let result = true;

    for (const feature of this._editData.features || []) {
      if (!this.isFeatureSelected(feature.name)) {
        continue;
      }

      const value = this.getFeatureValueByName(feature.name);
      if (!this.isFeatureValueValid(feature.name, value)) {
        result = false;
      }
    }

    return result;
  }

  setFeatureValueByName(featureName: string, value: string): void {
    const featureValue = this._editData.featureValues?.find(f => f.name === featureName);
    if (!featureValue) {
      return;
    }

    featureValue.value = value;
  }

  isFeatureSelected(name: string): boolean {
    // let nodes = _.filter(this.selectedFeatures, { data: { name: name } });
    const nodes = this.selectedFeatures?.filter(o => o.data.name == name) || [];
    return nodes?.length === 1;
  }

  getFeatureValueByName(featureName: string): string {
    const feature = this._treeDataHelperService.findNode(this.treeData, { data: { name: featureName } });
    if (!feature) {
      return null;
    }

    if (!this.isFeatureSelected(featureName)) {
      return 'false';
    }

    if (feature.value) {
      return feature.value;
    }

    if (feature.data.inputType.name !== 'CHECKBOX') {
      return feature.data.defaultValue || 'true';
    }

    return 'true';
  }

  isFeatureEnabled(featureName: string): boolean {

    const value = this.findFeatureValueByName(featureName);
    return value.toLowerCase() === 'true';
  }

  nodeSelect(event) {
    const node = event.node;
    this.setDefaultValueIfNeeded(node);

    let parentNode = this._treeDataHelperService.findParent(this.treeData, { data: { name: node.data.name } });

    while (parentNode != null) {
      this.addToSelection(parentNode);
      this.setDefaultValueIfNeeded(parentNode);
      parentNode = this._treeDataHelperService.findParent(this.treeData, { data: { name: parentNode.data.name } });
    }

    this.setDefaultValuesForChildren(node);
  }

  private setDefaultValuesForChildren(node): void {
    if (!node.children) {
      return;
    }

    for (const child of node.children) {
      this.setDefaultValueIfNeeded(child);
      this.setDefaultValuesForChildren(child);
    }
  }

  onNodeUnselect(event) {
    const node = event.node;
    this.clearValueIfUnselected(node);
    this.selectedFeatures = this.selectedFeatures.filter(n => n.data.name !== node.data.name);
  }

  private clearValueIfUnselected(node): void {
    if (!this.isFeatureSelected(node.data.name) && node.data.inputType.name !== 'CHECKBOX') {
      node.value = undefined;
    }
    if (node.children) {
      for (const child of node.children) {
        this.clearValueIfUnselected(child);
      }
    }
  }
}