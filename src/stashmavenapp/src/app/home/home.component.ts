import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MsalService} from "@azure/msal-angular";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(
    private http: HttpClient){ }

  ngOnInit(): void {
    this.http.get('https://graph.microsoft.com/v1.0/me')
      .subscribe(
        profile => console.log(profile),
    );
  }
}
