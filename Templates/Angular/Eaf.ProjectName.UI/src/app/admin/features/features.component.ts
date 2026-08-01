import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EditionServiceProxy, FlatFeatureDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-features',
  templateUrl: './features.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class FeaturesComponent extends AppComponentBase implements OnInit {
  loading = false;
  features: FlatFeatureDto[] = [];
  filteredFeatures: FlatFeatureDto[] = [];
  filterText = '';

  constructor(
    injector: Injector,
    private readonly _editionService: EditionServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadFeatures();
  }

  loadFeatures(): void {
    this.loading = true;
    this._editionService
      .getAllFeatures()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(result => {
        this.features = result.items || [];
        this.filterFeatures();
      });
  }

  filterFeatures(): void {
    const text = this.filterText?.trim().toLowerCase();
    if (!text) {
      this.filteredFeatures = this.features;
      return;
    }

    this.filteredFeatures = this.features.filter(
      f =>
        (f.name || '').toLowerCase().includes(text) ||
        (f.displayName || '').toLowerCase().includes(text),
    );
  }
}
