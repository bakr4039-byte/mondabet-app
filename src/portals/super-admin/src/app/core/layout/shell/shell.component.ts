import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink,
    MatToolbarModule, MatSidenavModule, MatListModule,
    MatIconModule, MatButtonModule, MatMenuModule, TranslateModule,
  ],
  template: `
    <mat-sidenav-container style="height:100vh">
      <mat-sidenav mode="side" opened style="width:220px">
        <mat-toolbar color="primary" style="font-size:16px">Mondabet</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item routerLink="/tenants">
            <mat-icon matListItemIcon>business</mat-icon>
            <span matListItemTitle>{{ 'nav.tenants' | translate }}</span>
          </a>
          <a mat-list-item routerLink="/packages">
            <mat-icon matListItemIcon>inventory</mat-icon>
            <span matListItemTitle>{{ 'nav.packages' | translate }}</span>
          </a>
          <a mat-list-item routerLink="/reports">
            <mat-icon matListItemIcon>bar_chart</mat-icon>
            <span matListItemTitle>{{ 'nav.reports' | translate }}</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span style="flex:1"></span>
          <button mat-icon-button [matMenuTriggerFor]="langMenu">
            <mat-icon>language</mat-icon>
          </button>
          <mat-menu #langMenu>
            <button mat-menu-item (click)="setLang('ar')">عربي</button>
            <button mat-menu-item (click)="setLang('en')">English</button>
            <button mat-menu-item (click)="setLang('ur')">اردو</button>
          </mat-menu>
          <button mat-icon-button (click)="logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>
        <div style="padding:24px">
          <router-outlet />
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
})
export class ShellComponent {
  constructor(
    private translate: TranslateService,
    private router: Router,
  ) {}

  setLang(lang: string): void {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
    document.documentElement.dir = lang === 'ar' || lang === 'ur' ? 'rtl' : 'ltr';
    document.documentElement.lang = lang;
  }

  logout(): void {
    localStorage.removeItem('access_token');
    this.router.navigate(['/login']);
  }
}
