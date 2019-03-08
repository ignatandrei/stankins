import { TestBed } from '@angular/core/testing';

import { CronExecutionService } from './cron-execution.service';

describe('CronExecutionService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CronExecutionService = TestBed.get(CronExecutionService);
    expect(service).toBeTruthy();
  });
});
