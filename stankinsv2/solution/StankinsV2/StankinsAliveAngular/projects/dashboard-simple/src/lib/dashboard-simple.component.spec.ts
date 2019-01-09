import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardSimpleComponent } from './dashboard-simple.component';

describe('DashboardSimpleComponent', () => {
  let component: DashboardSimpleComponent;
  let fixture: ComponentFixture<DashboardSimpleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardSimpleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardSimpleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
