import { Injectable } from '@angular/core';
declare let alertify: any;
// we don't need to import it, it's already imported in angular.json
// just letting it know it's available and to consider it defined

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {
  constructor() {}

  confirm(message: string, okCallback: () => any) {
    // our dialog box to say 'are you sure you want to do this?'
    alertify.confirm(message, function(e) {
      if (e) {
        // e = is user clicks 'Ok'
        okCallback();
      } else {
        // the user clicks 'Cancel'
      }
    });
  }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }

  message(message: string) {
    alertify.message(message);
  }
}
