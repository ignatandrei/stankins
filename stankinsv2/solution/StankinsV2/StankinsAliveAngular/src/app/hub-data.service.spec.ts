import { TestBed } from '@angular/core/testing';

import { HubDataService } from './hub-data.service';

describe('HubDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: HubDataService = TestBed.get(HubDataService);
    expect(service).toBeTruthy();
  });
});
