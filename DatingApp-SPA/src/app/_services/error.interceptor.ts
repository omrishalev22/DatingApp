import { Injectable, ApplicationInitStatus } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  // requires an error interceptor provider in the app module (see below)
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        // goal here is to save some typing later on
        if (error instanceof HttpErrorResponse) {
          if (error.status === 401) {
            // e.g. wrong password
            return throwError(error.statusText);
          }
          const applicationError = error.headers.get('Application-Error');
          if (applicationError) {
            console.error(applicationError);
            return throwError(applicationError);
            // throwError is also an observalbe from rxjs
          }
          const serverError = error.error;
          let modalStateErrors = '';
          if (serverError && typeof serverError === 'object') {
            for (const key in serverError) {
              if (serverError[key]) {
                modalStateErrors += serverError[key] + '\n';
              }
            }
          }
          return throwError(modalStateErrors || serverError || 'Server Error');
          // this will accommodate all the different types of errors our API can throw,
          // passing info to our Angular client
        }
      })
    );
  }
}

// Angular has an array of HTTP_INTERCEPTORs, here we are adding one
// ErrorInterceptorProvider must also be added in app.module.ts under Providers
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
