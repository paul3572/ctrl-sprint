import { Component } from '@angular/core';
import { AppPaths } from '../app.paths';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './login.html',
  styleUrls: ['../app.css', './login.css'],
})
export class Login {
  protected readonly AppPaths = AppPaths;

  protected onLoginClicked() {}
}
