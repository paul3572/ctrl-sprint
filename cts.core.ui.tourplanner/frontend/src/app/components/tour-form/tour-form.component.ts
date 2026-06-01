import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Tour } from '../../models/tour';
import { Transport } from '../../models/transport';

@Component({
  selector: 'app-tour-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tour-form.component.html',
  styleUrls: ['./tour-form.component.scss'],
})
export class TourFormComponent {
  private readonly fb = inject(FormBuilder);

  @Input() isModal = true;
  @Output() tourSubmitted = new EventEmitter<Tour>();
  @Output() cancelled = new EventEmitter<void>();

  readonly submitting = signal(false);

  form: FormGroup;

  readonly transportTypes = Object.values(Transport);

  constructor() {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      from: ['', [Validators.required]],
      to: ['', [Validators.required]],
      transportType: [Transport.Foot, [Validators.required]],
      tourDistance: [0, [Validators.required, Validators.min(0.1)]],
      estimatedTimeMinutes: [0, [Validators.required, Validators.min(1)]],
    });
  }

  async onSubmit() {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.submitting.set(true);
    try {
      const newTour: Tour = {
        tourGuid: crypto.randomUUID(),
        name: this.form.value.name,
        description: this.form.value.description,
        from: this.form.value.from,
        to: this.form.value.to,
        transportType: this.form.value.transportType,
        tourDistance: parseFloat(this.form.value.tourDistance),
        estimatedTimeMinutes: parseInt(this.form.value.estimatedTimeMinutes, 10),
        rating: 0,
        tourLogs: [],
      };
      this.tourSubmitted.emit(newTour);
      this.form.reset();
    } finally {
      this.submitting.set(false);
    }
  }

  onCancel() {
    this.form.reset();
    this.cancelled.emit();
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    if (!control?.errors || !control?.touched) return '';

    if (control.errors['required']) return `${controlName} is required`;
    if (control.errors['minlength'])
      return `${controlName} must be at least ${control.errors['minlength'].requiredLength} characters`;
    if (control.errors['min']) return `${controlName} must be greater than ${control.errors['min'].min}`;

    return 'Invalid input';
  }
}

