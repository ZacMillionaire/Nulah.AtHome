export interface BasicEvent {
	Description: string;
	Id: number;
	Tags: string[] | null;
	Start: Date;
	End: Date | null;
}