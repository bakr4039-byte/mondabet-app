import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet />`,
})
export class AppComponent implements OnInit {
  constructor(private translate: TranslateService) {}

  ngOnInit(): void {
    this.translate.addLangs(['ar', 'en', 'ur']);
    this.translate.setDefaultLang('ar');
    const saved = localStorage.getItem('lang') ?? 'ar';
    this.translate.use(saved);
    this._setDir(saved);
  }

  private _setDir(lang: string): void {
    document.documentElement.dir = lang === 'ar' || lang === 'ur' ? 'rtl' : 'ltr';
    document.documentElement.lang = lang;
  }
}
