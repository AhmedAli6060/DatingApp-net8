import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_Services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  cancelRegister = output<boolean>();
  model: any = {};

  register() {
    if (!this.model.username) {
      this.toastr.warning('Please enter username');
    } else if (!this.model.password) {
      this.toastr.warning('Please enter password');
    } else {
      this.accountService.register(this.model).subscribe({
        next: (response) => {
          this.toastr.success('Register successfully');
          this.cancel();
        },
        error: (err) => {
          console.log(err.error);
          this.toastr.error(err.error);
        },
        complete: this.cancel,
      });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
