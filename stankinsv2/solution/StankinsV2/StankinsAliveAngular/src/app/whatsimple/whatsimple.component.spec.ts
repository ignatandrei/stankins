import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WhatsimpleComponent } from './whatsimple.component';

describe('WhatsimpleComponent', () => {
  let component: WhatsimpleComponent;
  let fixture: ComponentFixture<WhatsimpleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WhatsimpleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WhatsimpleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
