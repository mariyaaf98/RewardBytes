import { TestBed } from '@angular/core/testing';

import { Appreciation } from './appreciation';

describe('Appreciation', () => {
  let service: Appreciation;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Appreciation);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
