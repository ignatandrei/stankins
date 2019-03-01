import { TestBed } from '@angular/core/testing';

import { WritablesService } from './writables.service';

describe('WritablesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: WritablesService = TestBed.get(WritablesService);
    expect(service).toBeTruthy();
  });
});
