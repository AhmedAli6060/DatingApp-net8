import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_Services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  model: any = {};

  login() {
    console.log(this.model);
    if (!this.model.username) {
      this.toastr.warning('Please enter username');
    } else if (!this.model.password) {
      this.toastr.warning('Please enter password');
    } else {
      this.accountService.login(this.model).subscribe({
        next: (_) => {
          this.router.navigateByUrl('/members');
          this.model.username = this.model.password = '';
        },
        error: (err) => {
          console.log(err);
          this.toastr.error(err.error);
        }
      });
    }
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
