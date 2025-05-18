import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {AuthService} from '../services/auth.service';
@Injectable()
export class tokenInterceptor implements HttpInterceptor {

  intercept(req: HttpRequest<any>, handler: HttpHandler): Observable<HttpEvent<any>> {
    const authService = inject(AuthService);
    const router = inject(Router);
    const token =authService.getToken();

    if(token){
      req = req.clone({
        setHeaders: {Authorization:`Bearer ${token}`}  // "Bearer "+myToken
      })
    }
    return handler.handle(req);
  }

}
