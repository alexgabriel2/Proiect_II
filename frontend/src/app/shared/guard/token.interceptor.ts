import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable()
export class tokenInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, handler: HttpHandler): Observable<HttpEvent<any>> {
    const authService = inject(AuthService);
    const router = inject(Router);
    const token = authService.getToken();

    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return handler.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Token expired, attempt to refresh
          return authService.refreshToken().pipe(
            switchMap((response) => {
              const newToken = response.newToken;
              authService.setToken(newToken); // Save the new token
              const clonedRequest = req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` }
              });
              return handler.handle(clonedRequest);
            }),
            catchError((refreshError) => {
              // Refresh token failed, redirect to login
              router.navigate(['/login']);
              return throwError(refreshError);
            })
          );
        }
        return throwError(error);
      })
    );
  }
}
