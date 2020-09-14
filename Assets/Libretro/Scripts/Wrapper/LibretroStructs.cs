/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System;
using System.Runtime.InteropServices;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private struct retro_vfs_file_handle { }
        private struct retro_vfs_dir_handle { }

        [StructLayout(LayoutKind.Sequential)]
        private class retro_vfs_interface
        {
            public IntPtr get_path;        // retro_vfs_get_path_t
            public IntPtr open;            // retro_vfs_open_t
            public IntPtr close;           // retro_vfs_close_t
            public IntPtr size;            // retro_vfs_size_t
            public IntPtr tell;            // retro_vfs_tell_t
            public IntPtr seek;            // retro_vfs_seek_t
            public IntPtr read;            // retro_vfs_read_t
            public IntPtr write;           // retro_vfs_write_t
            public IntPtr flush;           // retro_vfs_flush_t
            public IntPtr remove;          // retro_vfs_remove_t
            public IntPtr rename;          // retro_vfs_rename_t
            public IntPtr truncate;        // retro_vfs_truncate_t
            public IntPtr stat;            // retro_vfs_stat_t
            public IntPtr mkdir;           // retro_vfs_mkdir_t
            public IntPtr opendir;         // retro_vfs_opendir_t
            public IntPtr readdir;         // retro_vfs_readdir_t
            public IntPtr dirent_get_name; // retro_vfs_dirent_get_name_t
            public IntPtr dirent_is_dir;   // retro_vfs_dirent_is_dir_t
            public IntPtr closedir;        // retro_vfs_closedir_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_vfs_interface_info
        {
            public uint required_interface_version;
            public retro_vfs_interface iface;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_hw_render_interface
        {
            public retro_hw_render_interface_type interface_type;
            public uint interface_version;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_led_interface
        {
            public IntPtr set_led_state; // retro_set_led_state_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_midi_interface
        {
            public IntPtr input_enabled;  // retro_midi_input_enabled_t
            public IntPtr output_enabled; // retro_midi_output_enabled_t
            public IntPtr read;           // retro_midi_read_t
            public IntPtr write;          // retro_midi_write_t
            public IntPtr flush;          // retro_midi_flush_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_hw_render_context_negotiation_interface
        {
            public retro_hw_render_context_negotiation_interface_type interface_type;
            public uint interface_version;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_memory_descriptor
        {
            public ulong flags;
            public void* ptr;
            public ulong offset;
            public ulong start;
            public ulong select;
            public ulong disconnect;
            public ulong len;
            public char* addrspace;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_memory_map
        {
            public retro_memory_descriptor* descriptors;
            public uint num_descriptors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_controller_description
        {
            public char* desc;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_controller_info
        {
            public retro_controller_description* types;
            public uint num_types;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_subsystem_memory_info
        {
            public char* extension;
            public uint type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_subsystem_rom_info
        {
            public char* desc;
            public char* valid_extensions;
            [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;
            [MarshalAs(UnmanagedType.U1)] public bool block_extract;
            [MarshalAs(UnmanagedType.U1)] public bool required;
            public retro_subsystem_memory_info* memory;
            public uint num_memory;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_subsystem_info
        {
            public char* desc;
            public char* ident;
            public retro_subsystem_rom_info* roms;
            public uint num_roms;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_get_proc_address_interface
        {
            public IntPtr get_proc_address; // retro_get_proc_address_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_log_callback
        {
            public IntPtr log; // retro_log_printf_t
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_perf_counter
        {
            public char* ident;
            public ulong start;
            public ulong total;
            public ulong call_cnt;

            [MarshalAs(UnmanagedType.U1)] public bool registered;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_perf_callback
        {
            public IntPtr get_time_usec;    // retro_perf_get_time_usec_t
            public IntPtr get_cpu_features; // retro_get_cpu_features_t

            public IntPtr get_perf_counter; // retro_perf_get_counter_t
            public IntPtr perf_register;    // retro_perf_register_t
            public IntPtr perf_start;       // retro_perf_start_t
            public IntPtr perf_stop;        // retro_perf_stop_t
            public IntPtr perf_log;         // retro_perf_log_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_camera_callback
        {
            public ulong caps;
            public uint width;
            public uint height;

            public IntPtr start;                 // retro_camera_start_t
            public IntPtr stop;                  // retro_camera_stop_t
            public IntPtr frame_raw_framebuffer; // retro_camera_frame_raw_framebuffer_t
            public IntPtr frame_opengl_texture;  // retro_camera_frame_opengl_texture_t
            public IntPtr initialized;           // retro_camera_lifetime_status_t
            public IntPtr deinitialized;         // retro_camera_lifetime_status_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_location_callback
        {
            public IntPtr start;         // retro_location_start_t
            public IntPtr stop;          // retro_location_stop_t
            public IntPtr get_position;  // retro_location_get_position_t
            public IntPtr set_interval;  // retro_location_set_interval_t

            public IntPtr initialized;   // retro_location_lifetime_status_t
            public IntPtr deinitialized; // retro_location_lifetime_status_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_rumble_interface
        {
            public IntPtr set_rumble_state; // retro_set_rumble_state_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_audio_callback
        {
            public IntPtr callback;  // retro_audio_callback_t
            public IntPtr set_state; // retro_audio_set_state_callback_t
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_frame_time_callback
        {
            public IntPtr callback; // retro_frame_time_callback_t
            public long reference;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_hw_render_callback
        {
            public retro_hw_context_type context_type;
            public IntPtr context_reset;           // retro_hw_context_reset_t
            public IntPtr get_current_framebuffer; // retro_hw_get_current_framebuffer_t
            public IntPtr get_proc_address;        // retro_hw_get_proc_address_t
            [MarshalAs(UnmanagedType.U1)] public bool depth;
            [MarshalAs(UnmanagedType.U1)] public bool stencil;
            [MarshalAs(UnmanagedType.U1)] public bool bottom_left_origin;
            public uint version_major;
            public uint version_minor;
            [MarshalAs(UnmanagedType.U1)] public bool cache_context;

            public IntPtr context_destroy; // retro_hw_context_reset_t

            [MarshalAs(UnmanagedType.U1)] public bool debug_context;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_keyboard_callback
        {
            public IntPtr callback; // retro_keyboard_event_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_disk_control_callback
        {
            public IntPtr set_eject_state; // retro_set_eject_state_t
            public IntPtr get_eject_state; // retro_get_eject_state_t

            public IntPtr get_image_index; // retro_get_image_index_t
            public IntPtr set_image_index; // retro_set_image_index_t
            public IntPtr get_num_images;  // retro_get_num_images_t

            public IntPtr replace_image_index; // retro_replace_image_index_t
            public IntPtr add_image_index;     // retro_add_image_index_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_disk_control_ext_callback
        {
            public IntPtr set_eject_state;     // retro_set_eject_state_t
            public IntPtr get_eject_state;     // retro_get_eject_state_t

            public IntPtr get_image_index;     // retro_get_image_index_t
            public IntPtr set_image_index;     // retro_set_image_index_t
            public IntPtr get_num_images;      // retro_get_num_images_t

            public IntPtr replace_image_index; // retro_replace_image_index_t
            public IntPtr add_image_index;     // retro_add_image_index_t

            public IntPtr set_initial_image;   // retro_set_initial_image_t

            public IntPtr get_image_path;      // retro_get_image_path_t
            public IntPtr get_image_label;     // retro_get_image_label_t
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_message
        {
            public char* msg;
            public uint frames;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_input_descriptor
        {
            public uint port;
            public uint device;
            public uint index;
            public uint id;
            public char* desc;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_system_info
        {
            public char* library_name;
            public char* library_version;
            public char* valid_extensions;
            [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;
            [MarshalAs(UnmanagedType.U1)] public bool block_extract;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_game_geometry
        {
            public uint base_width;
            public uint base_height;
            public uint max_width;
            public uint max_height;
            public float aspect_ratio;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_timing
        {
            public double fps;
            public double sample_rate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_av_info
        {
            public retro_game_geometry geometry;
            public retro_system_timing timing;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_variable
        {
            public char* key;
            public char* value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_option_display
        {
            public char* key;
            [MarshalAs(UnmanagedType.U1)] public bool visible;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_option_value
        {
            public char* value;
            public char* label;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_option_definition
        {
            public char* key;
            public char* desc;
            public char* info;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = RETRO_NUM_CORE_OPTION_VALUES_MAX)]
            public retro_core_option_value[] values; // retro_core_option_value[RETRO_NUM_CORE_OPTION_VALUES_MAX]
            public char* default_value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct retro_core_options_intl
        {
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)]
            public IntPtr us;    // retro_core_option_definition*
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)]
            public IntPtr local; // retro_core_option_definition*
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct retro_game_info
        {
            public char* path;
            public void* data;
            public uint size;
            public char* meta;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct retro_framebuffer
        {
            public IntPtr data; // void*
            public uint width;
            public uint height;
            public uint pitch;
            public retro_pixel_format format;
            public uint access_flags;
            public uint memory_flags;
        }
    }
}
