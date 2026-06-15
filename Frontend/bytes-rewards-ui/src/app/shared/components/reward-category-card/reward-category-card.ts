import { Component, input, output } from '@angular/core';
import { RewardCategory } from '../../../core/models/reward-category';

@Component({
  selector: 'app-reward-category-card',
  standalone: true,
  imports: [],
  templateUrl: './reward-category-card.html',
  styleUrl: './reward-category-card.css',
})
export class RewardCategoryCardComponent {
  category = input.required<RewardCategory>();
  change    = output<void>();

  getInitial(name: string): string {
    return name?.charAt(0)?.toUpperCase() ?? '?';
  }
}
