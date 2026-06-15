import {
  Component,
  computed,
  effect,
  ElementRef,
  EventEmitter,
  HostListener,
  input,
  OnDestroy,
  OnInit,
  Output,
  signal,
  viewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RewardCategory } from '../../../core/models/reward-category';

@Component({
  selector: 'app-reward-category-picker-modal',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './reward-category-picker-modal.html',
  styleUrl: './reward-category-picker-modal.css',
})
export class RewardCategoryPickerModalComponent implements OnInit, OnDestroy {
  // ── Inputs ────────────────────────────────────────────────────
  categories = input.required<RewardCategory[]>();
  selectedId  = input<string>('');

  // ── Outputs ───────────────────────────────────────────────────
  @Output() categorySelected = new EventEmitter<RewardCategory>();
  @Output() closed            = new EventEmitter<void>();

  // ── Refs ──────────────────────────────────────────────────────
  private searchInput = viewChild<ElementRef<HTMLInputElement>>('searchInput');
  private listRef     = viewChild<ElementRef<HTMLUListElement>>('listRef');

  // ── State (signals) ───────────────────────────────────────────
  readonly searchQuery    = signal('');
  readonly focusedIndex   = signal(-1);

  readonly filteredCategories = computed(() => {
    const q = this.searchQuery().toLowerCase().trim();
    if (!q) return this.categories();
    return this.categories().filter(
      c =>
        c.name.toLowerCase().includes(q) ||
        c.description?.toLowerCase().includes(q)
    );
  });

  // Auto-reset focused index when filter changes
  private readonly _resetFocus = effect(() => {
    this.filteredCategories(); // track
    this.focusedIndex.set(-1);
  });

  // ── Lifecycle ─────────────────────────────────────────────────
  ngOnInit(): void {
    // Prevent body scroll while modal is open
    document.body.style.overflow = 'hidden';
    // Focus search input after render
    setTimeout(() => this.searchInput()?.nativeElement.focus(), 50);
  }

  ngOnDestroy(): void {
    document.body.style.overflow = '';
  }

  // ── Keyboard navigation ───────────────────────────────────────
  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const items = this.filteredCategories();

    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        this.focusedIndex.update(i => Math.min(i + 1, items.length - 1));
        this.scrollToFocused();
        break;

      case 'ArrowUp':
        e.preventDefault();
        this.focusedIndex.update(i => Math.max(i - 1, 0));
        this.scrollToFocused();
        break;

      case 'Enter':
        e.preventDefault();
        const fi = this.focusedIndex();
        if (fi >= 0 && fi < items.length) {
          this.select(items[fi]);
        }
        break;

      case 'Escape':
        e.preventDefault();
        this.close();
        break;
    }
  }

  private scrollToFocused(): void {
    const list = this.listRef()?.nativeElement;
    if (!list) return;
    const item = list.querySelector<HTMLLIElement>(
      `[data-index="${this.focusedIndex()}"]`
    );
    item?.scrollIntoView({ block: 'nearest' });
  }

  // ── Actions ───────────────────────────────────────────────────
  select(cat: RewardCategory): void {
    this.categorySelected.emit(cat);
    this.closed.emit();
  }

  close(): void {
    this.closed.emit();
  }

  onBackdropClick(e: MouseEvent): void {
    if ((e.target as HTMLElement).classList.contains('picker-backdrop')) {
      this.close();
    }
  }

  clearSearch(): void {
    this.searchQuery.set('');
    this.searchInput()?.nativeElement.focus();
  }

  trackById(_: number, cat: RewardCategory): string {
    return cat.id;
  }

  getInitial(name: string): string {
    return name?.charAt(0)?.toUpperCase() ?? '?';
  }
}
