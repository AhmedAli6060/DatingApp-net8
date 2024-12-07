import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_Services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  cancelRegister = output<boolean>();
  model: any = {};

  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        alert('Register successfully');
        console.log(response);
        this.cancel();
      },
      error: (err) => {
        console.log(err.error)
        alert('Some thing went wrong! \n' + err.error);
      },
      complete: this.cancel,
    });
    return true;
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}

