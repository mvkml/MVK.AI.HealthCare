export type Persona = 'doctor' | 'patient';

export interface AuthUser {
  id: string;
  fullName: string;
  email: string;
  persona: Persona;
}

export interface RoleOption {
  persona: Persona;
  label: string;
}

export interface AuthResult {
  isNotValid: boolean;
  message: string;
  user?: AuthUser;
}
