import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  templateUrl: './modal.html',
  styleUrl: './modal.css'
})
export class ModalComponent {

  @Input() isOpen = false;

  @Output() close = new EventEmitter<void>();

  onClose() {
    this.close.emit();
  }

}