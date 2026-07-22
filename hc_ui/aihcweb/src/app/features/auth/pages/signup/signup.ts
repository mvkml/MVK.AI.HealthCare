import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService, ROLE_OPTIONS } from '../../data/auth.service';
import { Persona } from '../../models/auth.model';

@Component({
  selector: 'app-signup',
  imports: [FormsModule, RouterLink],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Signup {
  readonly roleOptions = ROLE_OPTIONS;

  readonly fullName = signal('');
  readonly email = signal('');
  readonly persona = signal<Persona>('doctor');
  readonly password = signal('');
  readonly confirmPassword = signal('');

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly passwordStrength = computed(() => {
    const p = this.password();
    if (!p) return { label: '', color: 'transparent', width: '0%' };
    if (p.length < 8) return { label: 'Weak', color: '#c0524a', width: '33%' };
    if (p.length < 12) return { label: 'Medium', color: '#b8860b', width: '66%' };
    return { label: 'Strong', color: '#2f8f5b', width: '100%' };
  });

  constructor(private readonly auth: AuthService) {}

  onSubmit(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (!this.fullName().trim() || !this.email().trim() || !this.password() || !this.confirmPassword()) {
      this.errorMessage.set('Please fill in all fields.');
      return;
    }
    if (this.password() !== this.confirmPassword()) {
      this.errorMessage.set('Passwords do not match.');
      return;
    }
    if (this.password().length < 8) {
      this.errorMessage.set('Password must be at least 8 characters.');
      return;
    }

    this.isLoading.set(true);
    this.auth.signUp(this.fullName().trim(), this.email(), this.password(), this.persona()).subscribe((result) => {
      this.isLoading.set(false);
      if (result.isNotValid) {
        this.errorMessage.set(result.message);
        return;
      }
      this.successMessage.set(result.message);
    });
  }
}
