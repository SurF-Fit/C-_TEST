import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UploadComponent } from './components/upload/upload.component';
import { ResultsComponent } from './components/results/results.component';
import { ValuesComponent } from './components/values/values.component';

const routes: Routes = [
    { path: '', redirectTo: '/upload', pathMatch: 'full' },
    { path: 'upload', component: UploadComponent },
    { path: 'results', component: ResultsComponent },
    { path: 'values', component: ValuesComponent },
    { path: '**', redirectTo: '/upload' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }