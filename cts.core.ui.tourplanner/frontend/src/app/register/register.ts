import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppPaths } from '../app.paths';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './register.html',
  styleUrls: ['../app.css', './register.css'],
})
export class Register {
  protected readonly AppPaths = AppPaths;

  protected onRegisterClicked() {}
}
