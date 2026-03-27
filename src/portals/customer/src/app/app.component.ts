import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet />`,
})
export class AppComponent implements OnInit {
  constructor(
    private translate: TranslateService,
    private http: HttpClient,
  ) {}

  ngOnInit(): void {
    this.translate.addLangs(['ar', 'en', 'ur']);
    const saved = localStorage.getItem('lang') ?? 'ar';
    this.translate.use(saved);
    this._setDir(saved);
    this._applyTenantTheme();
  }

  private _setDir(lang: string): void {
    document.documentElement.dir = lang === 'ar' || lang === 'ur' ? 'rtl' : 'ltr';
    document.documentElement.lang = lang;
  }

  private _applyTenantTheme(): void {
    // Fetch tenant config and patch CSS variables for white-label theming
    this.http
      .get<{ primaryColor: string; secondaryColor: string }>(
        `${environment.apiUrl}/tenants/config`,
      )
      .subscribe({
        next: (cfg) => {
          document.documentElement.style.setProperty('--primary', cfg.primaryColor);
          document.documentElement.style.setProperty('--secondary', cfg.secondaryColor);
        },
        error: () => {/* use defaults */},
      });
  }
}
