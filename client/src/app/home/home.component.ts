import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';
import { UserViewModel } from '../_models/UserViewModel';
import { AppUsers } from '../_models/AppUser';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
 
  
  http = inject(HttpClient);
  users: AppUsers[] = [];
  registerMode = false;
  
  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event:boolean){
    this.registerMode=event;
  }

  getUsers() {
    this.http.get<UserViewModel>('https://localhost:5001/api/Users').subscribe({
      next: (response) => {
        this.users = response.users;
      },
      error: (err) => {
        console.error(err)
      },
      complete: () => console.log('Request has completed'),
    });
  }


}
