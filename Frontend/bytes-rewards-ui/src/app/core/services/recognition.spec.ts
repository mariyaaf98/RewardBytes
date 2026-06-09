import { TestBed } from '@angular/core/testing';

import { Recognition } from './recognition';

describe('Recognition', () => {
  let service: Recognition;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Recognition);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
