import {Component, Input} from '@angular/core';
import { CommonModule } from '@angular/common';
import {Partner} from "../partners.service";
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-partner-preview',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './partner-preview.component.html',
  styleUrls: ['./partner-preview.component.css']
})
export class PartnerPreviewComponent {
  @Input() partner?: Partner | null;
}
