import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_Services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { DatePipe } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [DatePipe,TabsModule,GalleryModule,TimeagoModule],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css',
})
export class MemberDetailsComponent implements OnInit{
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member?:Member;
  images:GalleryItem[]=[];
 
  ngOnInit(): void {
      this.loadMember();
  }

  loadMember(){
    const username = this.route.snapshot.paramMap.get('username')
    if(!username) return;
    this.memberService.getMember(username).subscribe({
      next:member => {
        this.member=member,
        member.photos.map(p=>{
          this.images.push(new ImageItem({src:p.url,thumb:p.url}))
        })
      }
    })

  }
  
}
