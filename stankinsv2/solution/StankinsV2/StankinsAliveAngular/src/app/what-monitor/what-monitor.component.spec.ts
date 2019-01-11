import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WhatMonitorComponent } from './what-monitor.component';

describe('WhatMonitorComponent', () => {
  let component: WhatMonitorComponent;
  let fixture: ComponentFixture<WhatMonitorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WhatMonitorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WhatMonitorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
