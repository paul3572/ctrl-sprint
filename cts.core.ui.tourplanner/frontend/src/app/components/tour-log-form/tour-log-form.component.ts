import {
  Component,
  EventEmitter,
  Input,
  Output,
  inject,
  signal,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TourLog } from '../../models/tourLog';

@Component({
  selector: 'app-tour-log-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tour-log-form.component.html',
  styleUrl: './tour-log-form.component.scss',
})
export class TourLogFormComponent {
  private readonly fb = inject(FormBuilder);

  @Input() tourGuid!: string;
  @Input() existingLog?: TourLog | null;

  @Output() logSubmitted = new EventEmitter<TourLog>();
  @Output() logUpdated = new EventEmitter<TourLog>();
  @Output() cancelled = new EventEmitter<void>();

  readonly submitting = signal(false);

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      dateTime: [new Date().toISOString().slice(0, 16), [Validators.required]],
      comment: [''],
      difficulty: [3, [Validators.required, Validators.min(1), Validators.max(5)]],
      totalDistance: [0, [Validators.required, Validators.min(0)]],
      totalTime: [0, [Validators.required, Validators.min(0)]],
      rating: [3, [Validators.required, Validators.min(1), Validators.max(5)]],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['existingLog'] && this.existingLog) {
      this.form.patchValue({
        dateTime: new Date(this.existingLog.dateTime).toISOString().slice(0, 16),
        comment: this.existingLog.comment || '',
        difficulty: this.existingLog.difficulty || 3,
        totalDistance: this.existingLog.totalDistance ?? 0,
        totalTime: this.existingLog.totalTime ?? 0,
        rating: this.existingLog.rating ?? 3,
      });
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.submitting.set(true);
    try {
      if (this.existingLog) {
        const updatedLog: TourLog = {
          tourLogGuid: this.existingLog.tourLogGuid,
          tourGuid: this.existingLog.tourGuid,
          dateTime: new Date(this.form.value.dateTime),
          comment: this.form.value.comment,
          difficulty: this.form.value.difficulty,
          totalDistance: parseFloat(this.form.value.totalDistance),
          totalTime: parseInt(this.form.value.totalTime, 10),
          rating: this.form.value.rating,
        };
        this.logUpdated.emit(updatedLog);
      } else {
        const newTourLog: TourLog = {
          tourLogGuid: crypto.randomUUID(),
          tourGuid: this.tourGuid,
          dateTime: new Date(this.form.value.dateTime),
          comment: this.form.value.comment,
          difficulty: this.form.value.difficulty,
          totalDistance: parseFloat(this.form.value.totalDistance),
          totalTime: parseInt(this.form.value.totalTime, 10),
          rating: this.form.value.rating,
        };
        this.logSubmitted.emit(newTourLog);
      }

      this.form.reset({
        dateTime: new Date().toISOString().slice(0, 16),
        comment: '',
        difficulty: 3,
        totalDistance: 0,
        totalTime: 0,
        rating: 3,
      });
      this.existingLog = null;
    } finally {
      this.submitting.set(false);
    }
  }

  onCancel(): void {
    this.form.reset({
      dateTime: new Date().toISOString().slice(0, 16),
      comment: '',
      difficulty: 3,
      totalDistance: 0,
      totalTime: 0,
      rating: 3,
    });
    this.cancelled.emit();
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
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
    if (control.errors['min'])
      return `${controlName} must be at least ${control.errors['min'].min}`;
    if (control.errors['max']) return `${controlName} must be at most ${control.errors['max'].max}`;

    return 'Invalid input';
  }
}
