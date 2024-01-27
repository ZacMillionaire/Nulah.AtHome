<script>
	import App from '../components/App.svelte'
	const nameBase = "farts";
	let name = nameBase;
	let count = 0;

	function UpdateName(){
		count++;
		name = `${nameBase}${count}`;
	}
</script>
<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>
<a href="/events">events</a>
<button on:click={UpdateName}>Change name</button>
<App name={name}/>
