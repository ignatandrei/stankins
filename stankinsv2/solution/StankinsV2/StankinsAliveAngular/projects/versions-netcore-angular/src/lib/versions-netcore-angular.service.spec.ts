import { TestBed } from '@angular/core/testing';

import { VersionsNetcoreAngularService } from './versions-netcore-angular.service';

describe('VersionsNetcoreAngularService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: VersionsNetcoreAngularService = TestBed.get(VersionsNetcoreAngularService);
    expect(service).toBeTruthy();
  });
});
