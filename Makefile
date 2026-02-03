# .PHONY:

default: build run

build: tiles.raw disk.img

run: 
	qemu-system-i386 \
	-drive format=raw,file=disk.img

clean:
	rm *.bin
	rm *.img
	rm raw/*.raw

load: 
	diskutil unmountDisk /dev/disk4
	sudo dd if=disk.img of=/dev/rdisk4 bs=512 status=progress
	sync
	diskutil eject /dev/disk4

# 1 sector
bootloader.bin: bootloader.asm
	nasm $< -o $@

# 59 sectors
kernel.bin: kernel.asm
	nasm $< -o $@
	truncate -s $$((512*59)) $@

# 64 sectors
source.bin: source.fs blocks.py
	python3 blocks.py $< $@
	truncate -s $$((512*64)) $@

disk.img: bootloader.bin kernel.bin source.bin
	cat bootloader.bin kernel.bin source.bin > disk.img

tiles.raw: tiles
	python3 format.py

truncate-%:
	@size=$$(stat -f %z $*); \
	newsize=$$(( (size + 511) / 512 * 512)); \
	truncate -s $$newsize $*
