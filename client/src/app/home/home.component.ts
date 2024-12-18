import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { AccountService } from '../_Services/account.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent  {
  private accountService = inject(AccountService);
  registerMode = false;
  
  isLoggedIn(){
    return this.accountService.currentUser()?.token ? true : false;
  }

  
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event:boolean){
    this.registerMode=event;
  }



}
