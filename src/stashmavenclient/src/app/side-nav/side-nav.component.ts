import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterLink, RouterLinkActive} from "@angular/router";

@Component({
  selector: 'app-side-nav',
  standalone: true,
    imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './side-nav.component.html',
})
export class SideNavComponent {

}
