import {CanActivateFn, Router} from '@angular/router';
import {AuthService} from '../services/auth.service';
import {inject} from '@angular/core';
import {map, Observable} from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService=inject(AuthService);
  const router=inject(Router);
  return authService.isLoggedIn().pipe(
    map((isLoggedIn: Object) => {
      if (isLoggedIn) {
        console.log(authService.getToken());
        return true;

      } else {
        router.navigate(['/login']);
        return false;
      }
    })
  ) as Observable<boolean>;
};
