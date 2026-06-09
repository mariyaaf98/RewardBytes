import { TestBed } from '@angular/core/testing';

import { Bytes } from './bytes';

describe('Bytes', () => {
  let service: Bytes;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Bytes);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
