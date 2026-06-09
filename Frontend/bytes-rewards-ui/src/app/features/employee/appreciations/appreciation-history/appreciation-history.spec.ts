import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppreciationHistory } from './appreciation-history';

describe('AppreciationHistory', () => {
  let component: AppreciationHistory;
  let fixture: ComponentFixture<AppreciationHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppreciationHistory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppreciationHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
