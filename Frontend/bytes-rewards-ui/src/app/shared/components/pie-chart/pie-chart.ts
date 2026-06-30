import {
  Component, Input, OnChanges, ViewChild,
  ElementRef, AfterViewInit, SimpleChanges
} from '@angular/core';
import { CommonModule } from '@angular/common';

export interface PieSlice {
  label: string;
  value: number;
  color: string;
}

@Component({
  selector: 'app-pie-chart',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="flex flex-col lg:flex-row items-center gap-8">

      <!-- CANVAS -->
      <div class="relative shrink-0">
        <canvas #canvas [width]="size" [height]="size"></canvas>
        <!-- centre label -->
        <div class="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
          <p class="text-2xl font-black text-slate-900">{{ total | number }}</p>
          <p class="text-[10px] font-bold uppercase tracking-wider text-slate-400 mt-0.5">{{ centreLabel }}</p>
        </div>
      </div>

      <!-- LEGEND -->
      <div class="flex flex-col gap-2.5 flex-1 min-w-0">
        @for (s of slices; track s.label; let i = $index) {
          <div class="flex items-center gap-3">
            <div class="w-3 h-3 rounded-sm shrink-0" [style.background]="s.color"></div>
            <div class="flex-1 min-w-0">
              <div class="flex items-center justify-between gap-2">
                <span class="text-sm font-semibold text-slate-800 truncate">{{ s.label }}</span>
                <span class="text-sm font-bold shrink-0" [style.color]="s.color">
                  {{ s.value | number }}
                </span>
              </div>
              <!-- mini bar -->
              <div class="mt-1 h-1.5 rounded-full bg-slate-100 overflow-hidden">
                <div class="h-full rounded-full transition-all duration-700"
                  [style.width.%]="total > 0 ? (s.value / total) * 100 : 0"
                  [style.background]="s.color">
                </div>
              </div>
            </div>
            <span class="text-xs text-slate-400 shrink-0 w-10 text-right">
              {{ total > 0 ? ((s.value / total) * 100 | number:'1.0-0') : 0 }}%
            </span>
          </div>
        }
      </div>

    </div>
  `
})
export class PieChartComponent implements AfterViewInit, OnChanges {
  @Input() slices:      PieSlice[] = [];
  @Input() size:        number     = 200;
  @Input() centreLabel: string     = 'bytes';

  @ViewChild('canvas') canvasRef!: ElementRef<HTMLCanvasElement>;

  get total(): number {
    return this.slices.reduce((s, x) => s + x.value, 0);
  }

  ngAfterViewInit(): void { this.draw(); }
  ngOnChanges(_: SimpleChanges): void { setTimeout(() => this.draw(), 0); }

  private draw(): void {
    const canvas = this.canvasRef?.nativeElement;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const w = this.size;
    const h = this.size;
    const cx = w / 2;
    const cy = h / 2;
    const outerR = (Math.min(w, h) / 2) - 8;
    const innerR = outerR * 0.58;   // donut hole

    ctx.clearRect(0, 0, w, h);

    if (this.total === 0) {
      // draw empty grey ring
      ctx.beginPath();
      ctx.arc(cx, cy, outerR, 0, Math.PI * 2);
      ctx.arc(cx, cy, innerR, 0, Math.PI * 2, true);
      ctx.fillStyle = '#e2e8f0';
      ctx.fill();
      return;
    }

    let startAngle = -Math.PI / 2;   // start at top

    this.slices.forEach(slice => {
      const angle = (slice.value / this.total) * 2 * Math.PI;

      // outer arc
      ctx.beginPath();
      ctx.moveTo(cx + innerR * Math.cos(startAngle), cy + innerR * Math.sin(startAngle));
      ctx.arc(cx, cy, outerR, startAngle, startAngle + angle);
      ctx.arc(cx, cy, innerR, startAngle + angle, startAngle, true);
      ctx.closePath();

      ctx.fillStyle = slice.color;
      ctx.fill();

      // thin white gap between slices
      ctx.strokeStyle = '#ffffff';
      ctx.lineWidth = 2;
      ctx.stroke();

      startAngle += angle;
    });
  }
}
