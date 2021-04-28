import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { homeRoute } from './routes';
import { AppService } from './services/app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = '';
  isProgressBarVisible = false;
  isBackButtonVisible = false;

  constructor(
    private router: Router,
    private appService: AppService,
    private cdRef : ChangeDetectorRef) {
    this.appService.changeMainTitleSource.subscribe(async msg => this.changeTitle(msg));
    this.appService.showMainProgressBarSource.subscribe(async show => this.showProgressBar(show));
    this.appService.showBackButtonSource.subscribe(async show => this.showBackButton(show));
  }

  onGoBackClick(): void {
    this.router.navigate([homeRoute]);
    this.appService.showBackButton(false);
  }

  private changeTitle(title: string): void {
    this.title = title;
  }

  private showProgressBar(isVisible: boolean): void {
    this.isProgressBarVisible = isVisible;
  }

  private showBackButton(show: boolean): void {
    if (show != this.isBackButtonVisible) {
      this.isBackButtonVisible = show;
      this.cdRef.detectChanges();
    }

    if (!show) {
      this.title = '';
    }
  }
}
