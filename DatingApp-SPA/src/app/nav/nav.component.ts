import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router) {}

  ngOnInit() {}
  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Logged in successfully.');
    }, this.alertify.error,
  () => {
    this.router.navigate(['/members']);
  });
  }

  logout() {
    localStorage.removeItem('token'); // can they be logged in to two sites simultaneously?
    this.alertify.message('Logged out.');
    this.router.navigate(['/home']);
  }

  loggedIn() {
    return this.authService.loggedIn();
  }
}
