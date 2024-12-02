import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  
  http = inject(HttpClient);
  title = 'DatingApp';

  users: User[] = []; 

  ngOnInit(): void {
    this.http.get<UserViewModel>('https://localhost:5001/api/Users')
      .subscribe({
        next: (response) => {
          this.users = response.users;
        },
        error: (err) => console.error(err),
        complete:()=>console.log('Request has completed')
      });
  }
  
  // users:any;
  // ngOnInit(): void {
  //   this.http
  //     .get('https://localhost:5001/api/Users')
  //     .subscribe({
  //       //next: (response) => (this.users = response),
  //       next: (response) => {
  //         console.log('API Response:', response); // Check the structure here
  //         this.users = Array.isArray(response) ? response : [];
  //       },
  //       error: (err) => console.log(err),
  //       complete: () => console.log('Request has completed'),
  //     });
  // }
}

interface User {
  id: number;
  userName: string;
}

interface UserViewModel {
  users: User[];
}