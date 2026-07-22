export interface AdminUser {
  id: string;
  fullName: string;
  email: string;
}

export interface AdminAuthResult {
  isNotValid: boolean;
  message: string;
  user?: AdminUser;
}
