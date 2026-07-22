import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AdminAuthMockService } from '../../data/admin-auth-mock.service';

@Component({
  selector: 'app-admin-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-login.html',
  styleUrl: './admin-login.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminLogin {
  readonly email = signal('');
  readonly password = signal('');
  readonly showPassword = signal(false);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  constructor(private readonly auth: AdminAuthMockService, private readonly router: Router) {}

  togglePassword(): void {
    this.showPassword.update((v) => !v);
  }

  onSubmit(): void {
    this.errorMessage.set(null);

    if (!this.email().trim() || !this.password()) {
      this.errorMessage.set('Please enter your email and password.');
      return;
    }

    this.isLoading.set(true);
    this.auth.login(this.email(), this.password()).subscribe((result) => {
      this.isLoading.set(false);
      if (result.isNotValid) {
        this.errorMessage.set(result.message);
        return;
      }
      this.router.navigate(['/admin/home']);
    });
  }
}
