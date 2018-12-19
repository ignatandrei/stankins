import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VersionsNetcoreAngularComponent } from './versions-netcore-angular.component';

describe('VersionsNetcoreAngularComponent', () => {
  let component: VersionsNetcoreAngularComponent;
  let fixture: ComponentFixture<VersionsNetcoreAngularComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VersionsNetcoreAngularComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VersionsNetcoreAngularComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
