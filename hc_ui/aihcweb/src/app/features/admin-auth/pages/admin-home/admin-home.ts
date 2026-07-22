import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AdminAuthMockService } from '../../data/admin-auth-mock.service';

@Component({
  selector: 'app-admin-home',
  imports: [],
  templateUrl: './admin-home.html',
  styleUrl: './admin-home.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminHome {
  private readonly auth = inject(AdminAuthMockService);
  private readonly router = inject(Router);

  readonly currentAdmin = this.auth.currentAdmin;

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/admin/login']);
  }
}
