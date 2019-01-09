import { TestBed } from '@angular/core/testing';

import { DashboardSimpleService } from './dashboard-simple.service';

describe('DashboardSimpleService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DashboardSimpleService = TestBed.get(DashboardSimpleService);
    expect(service).toBeTruthy();
  });
});
