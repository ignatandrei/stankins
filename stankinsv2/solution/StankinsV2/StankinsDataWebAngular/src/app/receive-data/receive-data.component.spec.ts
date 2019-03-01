import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveDataComponent } from './receive-data.component';

describe('ReceiveDataComponent', () => {
  let component: ReceiveDataComponent;
  let fixture: ComponentFixture<ReceiveDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReceiveDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReceiveDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
