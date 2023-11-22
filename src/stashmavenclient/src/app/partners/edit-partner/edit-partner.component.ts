import {Component, Input} from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-partner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './edit-partner.component.html',
  styleUrls: ['./edit-partner.component.css']
})
export class EditPartnerComponent {

  @Input() partnerId: string | null = null;


  ngOnInit() {
    console.log('partnerId', this.partnerId)
  }
}
