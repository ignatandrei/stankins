import { TestBed } from '@angular/core/testing';

import { VersionsService } from './versions.service';

describe('VersionsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: VersionsService = TestBed.get(VersionsService);
    expect(service).toBeTruthy();
  });
});
