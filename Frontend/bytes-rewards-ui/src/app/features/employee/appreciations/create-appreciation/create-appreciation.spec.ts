import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateAppreciation } from './create-appreciation';

describe('CreateAppreciation', () => {
  let component: CreateAppreciation;
  let fixture: ComponentFixture<CreateAppreciation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateAppreciation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateAppreciation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
